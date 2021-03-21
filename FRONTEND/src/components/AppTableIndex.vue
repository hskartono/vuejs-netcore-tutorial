<template>
    <div class="table-corner">
          <b-table id="my-table" stacked="md" head-variant="light" v-bind="$attrs" small>
            <template #top-row>
                <b-th></b-th>
                
                <b-th stacked-heading="Select All" v-if="showSelectAll">
                  <b-form-checkbox id="input-select-all" class="float-right" v-model="selectAll"></b-form-checkbox>
                </b-th>
            </template>
            <template v-slot:table-busys>
              <div class="text-center my-2">
                <b-spinner class="align-middle text-primary"></b-spinner>
                <strong class="ml-2">Loading...</strong>
              </div>
            </template>
            <template v-slot:cell(actions)="row" >
              <div class="div-actions">
                <span v-if="showActionButton">
                    <b-button size="sm"  variant="success"  :to="{path: '/purchaseorder/create' }" class="mr-2">
                    <b-icon-pencil></b-icon-pencil>
                    </b-button>
                    <b-overlay
                        :show="busy"
                        rounded
                        opacity="0.6"
                        spinner-small
                        spinner-variant="primary"
                        class="d-inline-block"
                        @hidden="onHidden"
                    >
                    <click-confirm>
                        <b-button size="sm" variant="danger" class="mr-2">
                        <b-icon-trash></b-icon-trash>
                        </b-button>
                    </click-confirm>
                    </b-overlay>
                    <b-button size="sm" variant="primary" class="mr-2 mt-md-2 mt-lg-0"  :to="{path: '/purchaseorder/detail' }" >
                    <b-icon-eye-fill></b-icon-eye-fill>
                    </b-button>
                    <b-button size="sm" variant="light" class="mr-2 mt-md-2 mt-lg-0 d-none">
                    <b-icon-printer-fill></b-icon-printer-fill>
                    </b-button>
                </span>
                <b-form-checkbox v-if="showSelectAll" class="d-inline" :id="'input-is-selected-' + (row.index + 1)" v-model="row.item.isPrint"></b-form-checkbox>
              </div>
            </template>
          </b-table>
        </div>
</template>
<script>
export default {
    props: ["options", "showActionButton", "showSelectAll", "isDataLoading"],
    data() {
        return {
            
        }
    },
    methods: {
        setSelected(value) {
            this.inputModel = value;
        },
        onHidden() {
            console.log("hide");
        }
    }
}
</script>
<style scoped>
</style>