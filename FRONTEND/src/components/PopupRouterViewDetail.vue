<template>
  <Portal to="app-popup">
    <PopupBaseDetail
      :label="label"
      :isShowCopyData="isShowCopyData"
      @copyDataMethod="copyDataAction"
      @close="$router.back()"
      :okTitle="okTitle"
      :cancelTitle="cancelTitle"
      :okVariant="okVariant"
      :cancelVariant="cancelVariant"
    >
      <slot name="backdrop" slot="backdrop"/>
      <slot/>
      <template #modal-left-button>
        <slot name="modal-left-button"></slot>
      </template>
    </PopupBaseDetail>
  </Portal>
</template>

<script>
import PopupBaseDetail from './PopupBaseDetail.vue';

export default {
  name: 'PopupRouterViewDetail',
  props: {
    label: {
      required: true,
      type: String,
    },
    copyDataMethod: {},
    isShowCopyData: {
      type: Boolean,
      default: true
    },
    okTitle: {
      default: 'Close',
      type: String,
    },
    cancelTitle: {
      default: 'Copy Data',
      type: String,
    },
    okVariant: {
      default: 'success',
      type: String,
    },
    cancelVariant: {
      default: 'secondary',
      type: String,
    },
  },
  components: {
    PopupBaseDetail,
  },
  methods : {
    copyDataAction(modalEvent, modl) {
      this.$emit("copyDataMethod", modalEvent);
    }
  }
};
</script>