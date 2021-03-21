<template>
    <div>
        <b-form-group :id="groupId" v-bind="$attrs">
            <template v-slot:label>{{ labelStr }} <span v-if="isRequired" class="required"></span></template>
            <v-select :options="options" multiple :value="inputModel" :filterable="true" v-model="inputModel" @input="$emit('update:model', inputModel)"></v-select>
            <b-form-invalid-feedback :id="feedbackId">
                {{ errorMessage }}
            </b-form-invalid-feedback>
        </b-form-group>
    </div>
</template>
<script>
export default {
    props: ["model", "name", "errorMessage", "options", "isEditable", "label", "isRequired"],
    data() {
        return {
            labelStr: this.label,
            value:"",
            inputModel: this.model,
            groupId : "fieldset-" + name,
            feedbackId : "input-" + name + "-live-feedback",
        }
    }, 
    methods: {
        setSelected(value) {
            this.inputModel = value;
        }
    }
}
</script>
<style scoped>
.input-group-text {
    font-size: 0.7rem !important;
}
</style>